﻿using System;
using System.Collections.Generic;
using Griesoft.AspNetCore.ReCaptcha.Configuration;
using Griesoft.AspNetCore.ReCaptcha.Extensions;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;

namespace Griesoft.AspNetCore.ReCaptcha.TagHelpers
{
    /// <summary>
    /// Adds a script tag, which will load the required reCAPTCHA JavaScript API. Can be added
    /// anywhere on your HTML page, but if you use a onload callback function you must place this
    /// after that callback to avoid race conditions.
    /// </summary>
    [HtmlTargetElement("recaptcha-script", TagStructure = TagStructure.WithoutEndTag)]
    public class RecaptchaScriptTagHelper : TagHelper
    {
        private const string RecaptchaScriptEndpoint = "https://www.google.com/recaptcha/api.js";

        private readonly RecaptchaSettings _settings;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="settings"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public RecaptchaScriptTagHelper(IOptionsMonitor<RecaptchaSettings> settings)
        {
            _ = settings ?? throw new ArgumentNullException(nameof(settings));

            _settings = settings.CurrentValue;
        }

        /// <summary>
        /// Set the rendering mode for the script.
        /// </summary>
        public Render Render { get; set; } = Render.Onload;

        /// <summary>
        /// Set the name of your callback function, that will be executed when reCAPTCHA has finished loading.
        /// </summary>
        public string? OnloadCallback { get; set; }

        /// <summary>
        /// Set a language code to force reCAPTCHA loading in the specified language. If not set language
        /// will be detected automatically by reCAPTCHA. For a list of valid language codes visit: 
        /// https://developers.google.com/recaptcha/docs/language
        /// </summary>
        public string? Language { get; set; }

        /// <inheritdoc/>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            _ = output ?? throw new ArgumentNullException(nameof(output));

            output.TagName = "script";
            output.TagMode = TagMode.StartTagAndEndTag;

            if (Render == Render.V3)
            {
                output.Attributes.Add("src", $"{RecaptchaScriptEndpoint}?render={_settings.SiteKey}");
            }
            else
            {
                var queryCollection = new Dictionary<string, string>();

                if (!string.IsNullOrEmpty(OnloadCallback))
                {
                    queryCollection.Add("onload", OnloadCallback);
                }

                if (Render == Render.Explicit)
                {
                    queryCollection.Add("render", Render.ToString().ToLowerInvariant());
                }

                if (!string.IsNullOrEmpty(Language))
                {
                    queryCollection.Add("hl", Language);
                }

                output.Attributes.SetAttribute("src", TagHelperOutputExtensions.AddQueryString(RecaptchaScriptEndpoint, queryCollection));
            }

            output.Attributes.SetAttribute("async", null);
            output.Attributes.SetAttribute("defer", null);

            output.Content.Clear();
        }
    }
}
