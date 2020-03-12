if (process.env.NODE_ENV === "production") {
    module.exports = {
        // connection to cloud mongodb server
        mongoURI: 'mongodb+srv://patrick:silva@cluster0-hdzrp.mongodb.net/test?retryWrites=true&w=majority'
    }
}
else {
    module.exports = {
        mongoURI: 'mongodb://localhost:27017/multiplayerserver'
    }
}