let buildId = process.env.BUILD_BUILDNUMBER;
var fs = require('fs');
let configPath = JSON.parse(fs.readFileSync('./mea-ui/build/config.json', 'utf8'));
configPath.version = buildId;
fs.writeFile('./mea-ui/build/config.json', JSON.stringify(configPath, null, 2), (err) => {
    if (err) {
        console.log(err);
        return;
    }
    console.log('config has been updated');
});
