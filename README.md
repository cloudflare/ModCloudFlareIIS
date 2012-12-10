IIS Module for CloudFlare users, similar to mod_cloudflare.c for Apache

-Restores original visitor IP address to web applications running on IIS and using CloudFlare's performance and security service.
-Adds original visitor IP address to logs

Tested with IIS 7, 7.5 and 8. Known to compile using .NET 2.0 or later.

More background on [seeing the original visitor IP address in logs when using CloudFlare](https://support.cloudflare.com/entries/22055137-why-do-my-server-logs-show-cloudflare-s-ips-using-cloudflare "CloudFlare"). More about [CloudFlare](https://www.cloudflare.com)

==Rough installation instruction==

1.) Compile, if needed.
2.) Copy both .dll files to the BIN folder in the root of your desired application/website folder. If this folder doesn't exist, then create it.
3.) In the IIS Manager, go to the website/application's "Modules" section.
4.) Right click, or use the interface to "Add Managed Module"
5.) Use "ModCloudFlare" as the name. (This is actually arbitrary.)
6.) For "Type" simply type in ModCloudFlareIIS.ModCloudFlare. (This should autocomplete.)
7.) Leave the checkbox alone.
8.) Press "OK."
9.) Restart the Website/Application, if necessary.

==Potential issues==

-This module is likely far from complete and may not cover all methods of retrieving a visitor's IP using ServerVariables
-IPv4 only, for now.


==Feedback==

Feedback encouraged, preferably via [CloudFlare Support](http://support.cloudflare.com).


This module uses the IPNetwork library by Luke Skywaler: ipnetwork.codeplex.com

Copyright (c) 2009, Luke Skywalker
All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

* Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.

* Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.

* Neither the name of Dark Industries nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

