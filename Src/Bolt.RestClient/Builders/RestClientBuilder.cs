using System.Collections.Generic;
using Bolt.Logger;
using Bolt.RestClient.Factories;
using Bolt.RestClient.Impl;
using Bolt.Serializer;

namespace Bolt.RestClient.Builders
{
    public class RestClientBuilder
    {
        private IErrorLoader _errorLoader;
        private ILogger _logger;
        private RestClientSettings _settings = new RestClientSettings();
        private IHttpWebRequestFactory _httpWebRequestFactory;
        private IHttpClientFactory _httpClientFactory;

        private RestClientBuilder()
        {
        }

        public RestClientBuilder WithHttpWebRequestFactory(IHttpWebRequestFactory factory)
        {
            _httpWebRequestFactory = factory;
            return this;
        }

        public RestClientBuilder WithHttpClientFactory(IHttpClientFactory factory)
        {
            _httpClientFactory = factory;
            return this;
        }

        public RestClientBuilder WithSettings(IRestClientSettings settings)
        {
            _settings = new RestClientSettings
            {
                AutomaticDecompression = settings.AutomaticDecompression,
                DefaultConnectionLimit = settings.DefaultConnectionLimit,
                BypassOnLocal = settings.BypassOnLocal,
                Proxy = settings.Proxy
            };

            return this;
        }

        public RestClientBuilder WithConnectionLimit(int limit)
        {
            _settings.DefaultConnectionLimit = limit;
            return this;
        }
        
        public RestClientBuilder WithAutomaticDecompression(bool automaticDecompression)
        {
            _settings.AutomaticDecompression = automaticDecompression;
            return this;
        }

        public RestClientBuilder WithProxy(string proxy, bool byPassOnLocal = false)
        {
            _settings.Proxy = proxy;
            _settings.BypassOnLocal = byPassOnLocal;
            return this;
        }

        public RestClientBuilder WithLogger(ILogger logger)
        {
            _logger = logger;
            return this;
        }

        #region filters

        private readonly List<IRequestFilter> _filters = new List<IRequestFilter>();
        
        public RestClientBuilder WithFilters(IEnumerable<IRequestFilter> filters)
        {
            _filters.AddRange(filters);

            return this;
        }

        public RestClientBuilder WithFilter(IRequestFilter filter)
        {
            _filters.Add(filter);

            return this;
        }

        #endregion

        #region serializers

        private readonly List<ISerializer> _serializers = new List<ISerializer>();

        public RestClientBuilder WithSerializers(IEnumerable<ISerializer> serializers)
        {
            _serializers.AddRange(serializers);

            return this;
        }

        public RestClientBuilder WithSerializer(ISerializer serializer)
        {
            _serializers.Add(serializer);

            return this;
        }

        #endregion
        
        #region TimeTakenNotifiers

        private readonly List<IReportTimeTaken> _timeTakenNotifiers = new List<IReportTimeTaken>();

        public RestClientBuilder WithTimeTakenNotifier(IReportTimeTaken timeTakenNotifier)
        {
            _timeTakenNotifiers.Add(timeTakenNotifier);
            return this;
        }

        public RestClientBuilder WithTimeTakenNotifiers(IEnumerable<IReportTimeTaken> timeTakenNotifiers)
        {
            _timeTakenNotifiers.AddRange(timeTakenNotifiers);
            return this;
        }

        #endregion

        #region interceptors

        private List<IRequestInterceptor> _interceptors;

        public RestClientBuilder WithInterceptors(IEnumerable<IRequestInterceptor> interceptors)
        {
            if(_interceptors == null) _interceptors = new List<IRequestInterceptor>();

            _interceptors.AddRange(interceptors);

            return this;
        }

        public RestClientBuilder WithInterceptor(IRequestInterceptor interceptor)
        {
            if (_interceptors == null) _interceptors = new List<IRequestInterceptor>();

            _interceptors.Add(interceptor);

            return this;
        }

        #endregion

        public RestClientBuilder WithErrorLoader(IErrorLoader errorLoader)
        {
            _errorLoader = errorLoader;

            return this;
        }

        public static RestClientBuilder New()
        {
            return new RestClientBuilder();
        }

        public IRestClient Build()
        {
            if(_logger == null) _logger = new NullLogger();
            if(_settings == null) _settings = new RestClientSettings();
            if(_errorLoader == null) _errorLoader = new ErrorLoader(_logger);
            if(_httpWebRequestFactory == null) _httpWebRequestFactory = new HttpWebRequestFactory(_settings, _logger);
            if(_httpClientFactory == null) _httpClientFactory = new HttpClientFactory(_settings);

            var requestExecutor = new RequestExecutor(new ExecutionTimeProfiler(_timeTakenNotifiers), _httpWebRequestFactory, _httpClientFactory, _logger, _filters, _interceptors, _serializers);
            var responseBuilder = new ResponseBuilder(_errorLoader, _serializers);
            var interceptorProvider = new InterceptorExecutor(_interceptors, responseBuilder, _logger);

            return new Impl.RestClient(requestExecutor, responseBuilder, _filters, interceptorProvider, _logger);
        }
    }
}