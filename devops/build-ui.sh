#!/bin/bash
OUTPUT_FOLDER=${1:dist}

#Enable NVM on the Azure DevOps Linux Build Host
. ${NVM_DIR}/nvm.sh

nvm install 22.13
nvm use 22.13

pwd

ls

printTask "Restoring npm packages..."
npm i --legacy-peer-deps # --unsafe-perm --verbose 

npm run version

printTask "Building main application..."

if [[ "${OUTPUT_FOLDER}" != "" ]]; then
mkdir -p $OUTPUT_FOLDER && chmod -R 777 $OUTPUT_FOLDER
npm run build -- --output-path $OUTPUT_FOLDER

else

npm run build

fi
