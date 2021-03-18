import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';
import firebase from 'firebase/app';
import { userMessagesPath } from '../../app/FirebasePaths';

export const sendMessage = createAsyncThunk('chat/sendMessage', (messagePayLoad, thunkApi) => {

    const newMessagePostRef = firebase.database().ref(`${userMessagesPath}/${messagePayLoad.userId}`).push();

    const newMessagePost = {
      ...messagePayLoad,
      id: newMessagePostRef.key,
      timeStampCreated: firebase.database.ServerValue.TIMESTAMP,
    };

    newMessagePostRef.set(messagePayLoad
      ,(error) => {
        if (error) {
          thunkApi.dispatch(messageError({ id: messagePayLoad.id }));
        } 
    });

    return newMessagePost;
});

export const chatSlice = createSlice({
    name: 'chat',
    initialState: {
        messagesIds: [],
        messages:{}
    },
    reducers: {
        messageSent: (state, action) => {
            const messageExists = state.messages[action.payload.id];

            // Means this was a message that was sent by the current user.
            if(messageExists){
                action.payload.sent = true;
            }

            state.messages[action.payload.id] = action.payload;
        },
        messageReceived: (state, action) => {
            const message = action.payload;
            const messageExists = message.id in state.messages;

            if(messageExists){
                message.sent = true;
            }
            else{
                state.messages[message.id] = message;
            }
        },
        messageError: (state, action) => {
            const message = action.payload;
            const messageExists = message.id in state.messages;

            if(messageExists){
                message.error = true;
            }
        },
        clearMessages: (state, action) => {
            state.messages = [];
        }
    },
    extraReducers: {
        [sendMessage.success]: (state, action) => {
            state.messagesIds.push(action.payload.id);
            state.messages[action.payload.id] = {...action.payload, timeStampCreated: (new Date()).getTime()};
        }
    } 
});

export const { addMessage, clearMessages, messageReceived, messageError } = chatSlice.actions;

export const selectMessages = state => state.chat.messageIds.map( id => state.chat.messages[id]);

export default chatSlice.reducer;

