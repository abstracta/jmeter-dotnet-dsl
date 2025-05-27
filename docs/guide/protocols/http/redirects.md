#### Redirects

When JMeter DSL (using JMeter logic) detects a redirection, it will automatically do a request to the redirected URL and register the redirection as a sub-sample of the main request.

If you want to disable such logic, you can just call `.FollowRedirects(false)` in a given `HttpSampler`.
