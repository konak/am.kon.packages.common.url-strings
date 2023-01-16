using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Text.Encodings.Web;
using am.kon.packages.common.strings;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;

namespace am.kon.packages.common.url_strings
{
    /// <summary>
    /// Extensiom methods for <see cref="string"/> objects containing URLs
    /// </summary>
	public static class UriStringExtensions
	{
        [DebuggerStepThrough]
        public static string EnsureLeadingSlash(this string url)
        {
            if (string.IsNullOrEmpty(url))
                return url;

            if (url[0] != Constants.Characters.SlashCharacter)
                return Constants.Characters.SlashCharacter + url;

            return url;
        }

        [DebuggerStepThrough]
        public static string EnsureTrailingSlash(this string url)
        {
            if (string.IsNullOrEmpty(url))
                return url;

            if (url[url.Length - 1] != Constants.Characters.SlashCharacter)
                return url + Constants.Characters.SlashCharacter;

            return url;
        }

        [DebuggerStepThrough]
        public static string RemoveLeadingSlash(this string url)
        {
            if (string.IsNullOrEmpty(url))
                return url;

            if (url[0] == Constants.Characters.SlashCharacter)
                return url.Substring(1);

            return url;
        }

        [DebuggerStepThrough]
        public static string RemoveTrailingSlash(this string url)
        {
            if (string.IsNullOrEmpty(url))
                return url;

            int endIndex = url.Length - 1;

            if (url[endIndex] == Constants.Characters.SlashCharacter)
                return url.Substring(0, endIndex);

            return url;
        }

        [DebuggerStepThrough]
        public static string CleanUrlPath(this string url)
        {
            if (string.IsNullOrWhiteSpace(url)) url = Constants.Strings.SlashString;

            int endIndex = url.Length - 1;

            if (url != Constants.Strings.SlashString && url[endIndex] == Constants.Characters.SlashCharacter)
                return url.Substring(0, endIndex);

            return url;
        }

        [DebuggerStepThrough]
        public static bool IsLocalUrl(this string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return false;

            // Allows "/" or "/foo" but not "//" or "/\".
            if (url[0] == Constants.Characters.SlashCharacter)
            {
                // url is exactly "/"
                if (url.Length == 1)
                    return true;

                // url doesn't start with "//" or "/\"
                if (url[1] != Constants.Characters.SlashCharacter && url[1] != Constants.Characters.BackSlashCharacter)
                    return true;

                return false;
            }

            // Allows "~/" or "~/foo" but not "~//" or "~/\".
            if (url[0] == Constants.Characters.TildeCharacter && url.Length > 1 && url[1] == Constants.Characters.SlashCharacter)
            {
                // url is exactly "~/"
                if (url.Length == 2)
                {
                    return true;
                }

                // url doesn't start with "~//" or "~/\"
                if (url[2] != Constants.Characters.SlashCharacter && url[2] != Constants.Characters.BackSlashCharacter)
                {
                    return true;
                }

                return false;
            }

            return false;
        }

        [DebuggerStepThrough]
        public static string AddQueryString(this string url, string query)
        {
            if (!url.Contains(Constants.Characters.QuestionCharacter))
                url += Constants.Characters.QuestionCharacter;
            else if (!url.EndsWith(Constants.Characters.QuestionCharacter))
                url += Constants.Characters.AmpersantCharacter;

            return url + query;
        }

        [DebuggerStepThrough]
        public static string AddQueryString(this string url, string name, string value)
        {
            return url.AddQueryString(name + Constants.Characters.EqualCharacter + UrlEncoder.Default.Encode(value));
        }

        [DebuggerStepThrough]
        public static string AddHashFragment(this string url, string query)
        {
            if (!url.Contains(Constants.Characters.SharpCharacter))
                url += Constants.Characters.SharpCharacter;

            return url + query;
        }

        [DebuggerStepThrough]
        public static NameValueCollection ReadQueryStringAsNameValueCollection(this string url)
        {
            if (url != null)
            {
                var idx = url.IndexOf(Constants.Characters.QuestionCharacter);

                if (idx >= 0)
                    url = url.Substring(idx + 1);

                Dictionary<string, StringValues> query = QueryHelpers.ParseNullableQuery(url);

                if (query != null)
                {
                    NameValueCollection res = new NameValueCollection(query.Count);

                    foreach (KeyValuePair<string, StringValues> record in query)
                        res.Add(record.Key, record.Value[0]);

                    return res;
                }
            }

            return new NameValueCollection();
        }

        public static string GetOrigin(this string url)
        {
            if (url == null)
                return null;

            Uri uri;

            try
            {
                uri = new Uri(url);
            }
            catch (Exception)
            {
                return null;
            }

            if (uri.Scheme == InternalConstants.HTTP_SCHEME || uri.Scheme == InternalConstants.HTTPS_SCHEME)
            {
                return $"{uri.Scheme}://{uri.Authority}";
            }

            return null;
        }
    }
}

