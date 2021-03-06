﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Authentication;
using ActiveLogin.Authentication.BankId.Api;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ActiveLogin.Authentication.BankId.AspNetCore
{
    public class BankIdAuthenticationBuilder
    {
        public AuthenticationBuilder AuthenticationBuilder { get; }
        public string Name { get; }

        private readonly List<Action<HttpClient>> _httpClientConfigurators = new List<Action<HttpClient>>();
        private readonly List<Action<HttpClientHandler>> _httpClientHandlerConfigurators = new List<Action<HttpClientHandler>>();

        public BankIdAuthenticationBuilder(AuthenticationBuilder authenticationBuilder, string name)
        {
            AuthenticationBuilder = authenticationBuilder;
            Name = name;

            var services = AuthenticationBuilder.Services;

            AddBankIdHttpClient(services, Name, _httpClientConfigurators, _httpClientHandlerConfigurators);

            ConfigureBankIdHttpClient(httpClient => httpClient.BaseAddress = BankIdUrls.ProdApiBaseUrl);
            ConfigureBankIdHttpClientHandler(httpClientHandler => httpClientHandler.SslProtocols = SslProtocols.Tls12);
        }

        public void ConfigureBankIdHttpClient(Action<HttpClient> configureHttpClient)
        {
            AuthenticationBuilder.Services.TryAddTransient<BankIdApiClient>();
            _httpClientConfigurators.Add(configureHttpClient);
        }

        public void ConfigureBankIdHttpClientHandler(Action<HttpClientHandler> configureHttpClientHandler)
        {
            AuthenticationBuilder.Services.TryAddTransient<BankIdApiClient>();
            _httpClientHandlerConfigurators.Add(configureHttpClientHandler);
        }
            
        private static void AddBankIdHttpClient(IServiceCollection services, string name, List<Action<HttpClient>> httpClientConfigurators, List<Action<HttpClientHandler>> httpClientHandlerConfigurators)
        {
            services.AddHttpClient<IBankIdApiClient, BankIdApiClient>(name, httpClient =>
                {
                    httpClientConfigurators.ForEach(configurator => configurator(httpClient));
                })
                .ConfigurePrimaryHttpMessageHandler(() =>
                {
                    var httpClientHandler = new HttpClientHandler();
                    httpClientHandlerConfigurators.ForEach(configurator => configurator(httpClientHandler));
                    return httpClientHandler;
                });

            services.TryAddTransient<IBankIdApiClient, BankIdApiClient>();
        }
    }
}