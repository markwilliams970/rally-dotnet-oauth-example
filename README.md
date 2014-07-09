rally-dotnet-oauth-example
====================

Example of using Rally OAuth with .NET

![App Screenshot](https://raw.githubusercontent.com/markwilliams970/rally-dotnet-oauth-example/master/img/screenshot2.png)

Video of Example in action:
http://screencast.com/t/rLPO4pzipcHX

## Dependencies
- [RestSharp](https://github.com/restsharp/RestSharp "RestSharp")
- [JSON.net](http://james.newtonking.com/json "JSON.net")


## Client Ids
To create a new Oauth integration, you will need to create a client id for your new application. You can create/modify clients at `https://rally1.rallydev.com/login/accounts/index.html#/clients`  The page will ask you to enter a name and redirect_uri for your client. After saving it will give you a `client_id` and `client_secret` to use in your app. Zuul's oauth URLs are:

* Authentication `https://rally1.rallydev.com/login/oauth2/auth`
* Token `https://rally1.rallydev.com/login/oauth2/auth`

## Workflow 

Go to `https://rally1.rallydev.com/login/accounts/index.html#/clients` and create a new Client for your application. Save the Client ID and Client Secret.

![ClientID_ClientSecret](https://raw.githubusercontent.com/markwilliams970/rally-dotnet-oauth-example/master/img/screenshot1.png)

To gain an access token for a user from your app first:

Instantiates Windows Form with a WebBrowser component, navigating to the following URL and parameters:

redirect to `https://rally1.rallydev.com/login/oauth2/auth`

Encode the following parameters onto the URL

* `state` a key to use to validate the auth token, typically a UUID. 
* `response_type` set to 'code'.
* `redirect_uri` this must match the URL you specified when creating your client id and secret. 
* `client_id` is the client_id that was created in Rally. 
* `scope` set to 'alm'.

For example: `https://rally1.rallydev.com/login/oauth2/auth?state=e347b102-6029-49b0-81d7-5089d846812e&response_type=code&redirect_uri=http%3A%2F%2Flocalhost%3A4567%2Foauth-redirect&client_id=817b4273628c415cddfd657ab7224582&scope=alm`

In the handler for the URL you redirect to you need to get the code from the parameters and POST to the token endpoint. 

You will recieve a JSON body with 'state' and 'code'. 

* `state` must match the 'state' you specified earlier.
* `token` is the auth token that you can exchange for an access token. 

To exchange the auth token for an access token, issues a POST request using RestSharp:

`POST to https://rally1.rallydev.com/login/oauth2/auth`

The BODY must contain the following parameters in URL/form encoded format.

* `code` the 'code' you recieved.
* `redirect_uri` must match the redirect_uri you specified earlier.
* `grant_type` set to 'authorization_code'.
* `client_id` set to your client id.
* `client_secret` set to your client secret.

Ensure you set the Content-Type header to "application/x-www-form-urlencoded".

On success a JSON body of: 

`{"id_token":"???","refresh_token":"8b829ee0a4cb45f6884f580433c98","expires_in":86400,"token_type":"Bearer","access_token":"Ovp8AQ4pRMADXUmzR44V5IFb8ViYUdxhYJkd123aQs"}`

Will be returned.

Set a request header with a key of ZSESSIONID and a value of `Ovp8AQ4pRMADXUmzR44V5IFb8ViYUdxhYJkd123aQs` (the access_token) on followup requests to Rally.

Followup RestSharp request to https://rally1.rallydev.com/slm/webservice/v2.0/hierarchicalrequirement sets these values and returns the results of a query for User Stories.
