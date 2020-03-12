var mongoose = require('mongoose')
var Schema = mongoose.Schema;

var playerSchema = new Schema({
    id:{
        type:String
    },
    name:{
        type:String
    },
    password:{
        type:String
    },
    online:{
        type:Boolean
    },
    kills:{
        type:Number
    }
})

mongoose.model('players', playerSchema)