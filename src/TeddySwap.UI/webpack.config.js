const path = require('path');

module.exports = {
    entry: './wwwroot/scripts/App.ts',
    module: {
        rules: [
            {
                test: /\.tsx?$/,
                use: 'ts-loader',
                exclude: /node_modules/,
            },
        ],
    },
    resolve: {
        extensions: ['.tsx', '.ts', '.js']
    },
    output: {
        filename: 'app.bundle.js',
        path: path.resolve(__dirname, 'wwwroot/dist/'),
    },
    experiments: {
        asyncWebAssembly: true,
        topLevelAwait: true,
        layers: true // optional, with some bundlers/frameworks it doesn't work without
    },
    devtool: false
};