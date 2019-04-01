# WebsocketSecure_Demo

## Workflow

First the user has to login. There are 3 users predefined:

```json
Username:"Richi", Password:"Admin1234"
Username:"Bert", Password:"Admin1234"
Username:"Kneidi", Password:"Admin1234"
```

After that, the two other commands can be used.

## Commands

Below there's a list of all 3 available commands.

### Login

```json
{"command":"login","data":{"Username":"Kneidi","Password":"Admin1234"}}
```

--> **RETURNS**: All available users with an online-indicator

```json
[
    {"Username":"Kneidi","Online":true},
    {"Username":"Bert","Online":false}
]
```

--> **BROADCASTS**: A message for all connected users indicating a new user is online

```json
{"Command":"new_online","Data":"Kneidi"}
```

### Send message

```json
{"command":"send_message","data":{"To":"Richi", "From":"Kneidi","Content":"Succccer!"}}
```

--> **RETURNS**: Nothing

--> Receiver receives: Message with Timestamp

```json
{"From":"Kneidi","To":"Richi","Content":"Succccer!","Timestamp":"2019-03-31T19:14:17.1953196+02:00"}
```

### Get messages by user

```json
{"command":"get_messages_by_user","data":"Richi"}
```

--> **RETURNS**: An Array containing all messages involving both users.

```json
[
    {"From":"Richi","To":"Kneidi","Content":"Hallo.","Timestamp":"2019-03-31T19:01:16.8662934+02:00"},
    {"From":"Kneidi","To":"Richi","Content":"Hallo.","Timestamp":"2019-03-31T19:02:16.8662942+02:00"},
    ...
]
```