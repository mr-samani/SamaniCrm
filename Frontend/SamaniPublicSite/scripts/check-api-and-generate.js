// import http from "http";
// import { exec } from "child_process";

// const url = "https://localhost:44343/openapi/v1.json";

// http.get(url, (res) => {
//   if (res.statusCode === 200) {
//     console.log("✅ OpenApi JSON is reachable. Generating...");
//     exec("set JAVA_OPTS=-Djavax.net.ssl.trustStoreType=Windows-ROOT -Dlog.level=error && java -jar tools/openapi-generator-cli-7.12.0.jar generate -g typescript-angular -o src/shared/service-proxies -c open-api-config.json --skip-validate-spec");
//   } else {
//     console.error(`❌ Can't read ${url}. Status: ${res.statusCode}`);
//   }
// }).on("error", () => {
//   console.error(`❌ ERROR: Can't connect to ${url}. Is your API running?`);
// });

// scripts/check-api-and-generate.js
process.env['NODE_TLS_REJECT_UNAUTHORIZED'] = '0';
const https = require('https');
const { exec } = require('child_process');

const url = 'https://localhost:44343/openapi/v1.json';

https
  .get(url, (res) => {
    if (res.statusCode === 200) {
      console.log('✅ OpenApi JSON is reachable. Generating...');
      exec(
        'java -Djavax.net.ssl.trustStoreType=Windows-ROOT -Dlog.level=error -jar ../SamaniCrm/tools/openapi-generator-cli-7.12.0.jar generate -g typescript-angular -o src/shared/service-proxies -c open-api-config.json --skip-validate-spec',
        (error, stdout, stderr) => {
          if (error) {
            console.error(`❌ Generation failed: ${error.message}`);
            return;
          }
          if (stderr) {
            console.warn(`⚠️ stderr: ${stderr}`);
          }
          console.log('✅ API generation complete.');
        },
      );
    } else {
      console.error(`❌ Can't read ${url}. Status: ${res.statusCode}`);
    }
  })
  .on('error', () => {
    console.error(`❌ ERROR: Can't connect to ${url}. Is your API running?`);
  });
