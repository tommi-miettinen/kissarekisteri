cd Frontend

npm install
npm run build

cp -r dist/* ../wwwroot/

cd ..

echo "Frontend built and copied to wwwroot successfully."