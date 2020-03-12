const socketPort = 3001;
const expressPort = 3000;
var io = require('socket.io')(process.env.PORT || socketPort)
const expressHB = require('express-handlebars');
const express = require('express')
const mongoose = require('mongoose')
const shortid = require('shortid')
const db = require('./utilities/database')
const app = express()
var players = []
var playerNames= []
// Mongo DB Connction
mongoose.connect(db.mongoURI, {
    useUnifiedTopology:true,
    useNewUrlParser:true
}).then(()=>{
    console.log("You have connected to the Database")
}).catch((err)=>{
    console.log("You've Got Error: " + err);
})

require('./models/Player')
var player = mongoose.model('players')

// Website
app.engine('handlebars', expressHB({defaultLayout:'main'}));
app.set('view engine', 'handlebars');

app.get('/', (req, res)=>{
    player.find({}).then((players)=>{
        players.sort((a,b)=>b.kills-a.kills)
        // cachedEntries.sort((a,b)=>b.highScore-a.highScore)
        res.render('players/index', {players:players})
    })
})


app.listen(process.env.PORT || expressPort, ()=>{console.log("Express Server connected")})

// Socket IO
console.log('Socket Server connected')
io.on('connection', (socket)=>{
    console.log('client connected')
    var thisClientId = shortid.generate()
    var thisClientName = ''
    
    socket.on('createAccountRequest',(data)=>{
        player.findOne({name:data.name})
        .then((user)=>
        {
            if(user)
            {
                socket.emit('errorMsg',{message:"ACFAE:Account already exists"})
                return
            }
            
            console.log("hello")
            // Account can be created
            var newUser = {
                id:thisClientId,
                name:data.name,
                password:data.password,
                online:data.online,
                kills:data.kills   
            }

            player(newUser).save().then(()=>{
                console.log('account created')
                socket.emit('accountCreated')
            })
        })
    })

    socket.on('loginRequest', (data)=>{
        player.findOne({name:data.name})
        .then((user)=>{
            if(!user)
            {
                socket.emit('errorMsg',{message:"LFNEX:No Such User Exists. Do you need to make an Account?"})
                return
            }else if(user.password != data.password){
                socket.emit('errorMsg',{message:"LFWP:Password does not match login provided"})
                return
            }
            // login was successful
            thisClientId = user.id
            user.online = true
            players.push(user.id)
            playerNames.push(user.name)
            
            socket.emit('loginSuccess', { id: thisClientId })
            user.save()
        })
    })

    socket.on('start', ()=>{
        // my events
        // spawn all newly joined players on the server
        socket.broadcast.emit('spawn', { id: thisClientId })
        socket.broadcast.emit('requestPosition')
        var i = 0;
        
        players.forEach((playerId) => {
            if (playerId == thisClientId)
                return;

            socket.emit('spawn', { id: playerId, name: playerNames[i] })
            console.log('spawning player: ' + playerId)
            i++
        })

    })
   
    socket.on('updateName',(data)=>{
        data.id = thisClientId;
        thisClientName = data.name;
    })

    socket.on('move', (data)=>{
        data.id = thisClientId
        socket.broadcast.emit('move', data)

    })

    // this will update everyone of the retrieved data
    socket.on('updatePosition', (data)=>{
        data.id = thisClientId
        socket.broadcast.emit('updatePosition', data)
    })

    socket.on('updateHealth', (data)=>{
        data.id = thisClientId;
        socket.broadcast.emit('updateHealth', data)
    })

    socket.on('updateKills', (data)=>{
        player.findOne({name:data.name}).then((user)=>{
            console.log(user.name + ' killed ' + user.kills + " users")
            user.kills++
            user.save()
        })       
    })

    socket.on('disconnect', (data)=>{
        console.log('player disconnected')
        data.id = thisClientId;
        players.splice(players.indexOf(thisClientId), 1);
        playerNames.splice(players.indexOf(thisClientName), 1);
        socket.broadcast.emit('disconnected', {id:thisClientId})
        player.findOne({id:thisClientId}).then((user)=>{
            user.online = false
            user.save()
        })
        
        //players.pop[thisClientId]
        //players--
    })
})