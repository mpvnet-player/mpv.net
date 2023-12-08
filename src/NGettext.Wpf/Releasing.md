Releasing
=========

 - Update the `.nuspec` file with a new version number.
 - Build in release mode
 - Invoke `PM> nuget pack`; no options needed.  But it will release the debug version, so make sure your build configuration match! Say `PM> nuget pack -Prop Configuration=Release`.
 - Sign the created package, for example by
   ```
   PM> nuget sign .\NGettext.Wpf.1.1.0-alpha.nupkg -Timestamper http://timestamp.digicert.com -CertificateFingerprint 79a047643b02e7b677d5d0a962bc02ac19e63ca8
   ```
 - Verify signing with
   ```
   PM> nuget verify -Signatures .\NGettext.Wpf.1.1.0-alpha.nupkg
   
   Verifying NGettext.Wpf.1.1.0-alpha
   C:\Git\ngettext-wpf\NGettext.Wpf\.\NGettext.Wpf.1.1.0-alpha.nupkg
   
   Signature Hash Algorithm: SHA256
   Signature type: Author
   Verifying the author primary signature with certificate: 
       Subject Name: CN=ACCURATECH ApS, O=ACCURATECH ApS, L=Holstebro, C=DK, SERIALNUMBER=27635652, OID.2.5.4.15=Private Organization, OID.1.3.6.1.4.1.311.60.2.1.3=DK
       SHA1 hash: 79A047643B02E7B677D5D0A962BC02AC19E63CA8
       SHA256 hash: A66690776D4B00270DAA40F0336E4EE8288D2B2F9F77E6B132B63D18F0F408FF
       Issued by: CN=DigiCert EV Code Signing CA (SHA2), OU=www.digicert.com, O=DigiCert Inc, C=US
       Valid from: 06-02-2018 01:00:00 to 09-02-2021 13:00:00
   
   Timestamp: 19-02-2019 12:32:52
   
   Verifying author primary signature's timestamp with timestamping service certificate: 
       Subject Name: CN=DigiCert SHA2 Timestamp Responder, O=DigiCert, C=US
       SHA1 hash: 400191475C98891DEBA104AF47091B5EB6D4CBCB
       SHA256 hash: FC834D5BFFDE31DBA5B79BF95F573F7953BCBF9156E8525163E828EB92EA8A93
       Issued by: CN=DigiCert SHA2 Assured ID Timestamping CA, OU=www.digicert.com, O=DigiCert Inc, C=US
       Valid from: 04-01-2017 01:00:00 to 18-01-2028 01:00:00
   
   
   Successfully verified package 'NGettext.Wpf.1.1.0-alpha'
   ```