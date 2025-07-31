<<<<<<< HEAD
﻿const path = require('path');

module.exports = {
    entry: './Scripts/React/index.js',
    output: {
        path: path.resolve(__dirname, 'wwwroot/js'),
        filename: 'bundle.js',
    },
    module: {
        rules: [{
            test: /\.(js|jsx)$/,
            exclude: /node_modules/,
            use: 'babel-loader',
        }],
    },
    resolve: {
        extensions: ['.js', '.jsx'],
    },
    mode: 'development',
};
=======
﻿const path = require('path');

module.exports = {
    entry: './Scripts/React/index.js',
    output: {
        path: path.resolve(__dirname, 'wwwroot/js'),
        filename: 'bundle.js',
    },
    module: {
        rules: [{
            test: /\.(js|jsx)$/,
            exclude: /node_modules/,
            use: 'babel-loader',
        }],
    },
    resolve: {
        extensions: ['.js', '.jsx'],
    },
    mode: 'development',
};
>>>>>>> b295d161dc62922848cb9705d6e50a596a092157
