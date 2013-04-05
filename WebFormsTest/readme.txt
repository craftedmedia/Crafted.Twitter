This library contains a web form control and an MVC HtmlHelper extension to add a twitter feed to your projects, it contains built in cacheing and both synchronous and asynchronous options.

To implement this you will have to add the following app settings;

<!-- Crafted.Twitter settings -->
<add key="HandlerPath" value="~/TwitterHandler/"/>
<add key="TwitterAPIKey" value="" />
<add key="TwitterSecretKey" value="" />
<add key="TwitterAccessToken" value="" />
<add key="TwitterSecretToken" value="" />
<add key="TwitterCacheEnabled" value="true"/>
<add key="TwitterCacheDuration" value="20"/>

You will need to generate the key and token values from your twitter account.