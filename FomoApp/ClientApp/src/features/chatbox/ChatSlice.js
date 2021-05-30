import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';
import { friendActivityPath, userMessagesPath } from '../../app/FireBasePaths';

import ChatStatusType from './ChatStatusType';
import MessageType  from './MessageType';
import firebase from 'firebase/app';

export const sendMessage = createAsyncThunk('chat/sendMessage', (messagePayload, thunkApi) => {

    const message = messagePayload.message;
    const ownerId = messagePayload.ownerId;
    const newMessagePostRef = firebase.database().ref(`${userMessagesPath}/${ownerId}`).push();

    const newMessagePost = {
      ...message,
      text: message.text,
      id: newMessagePostRef.key,
      timeStampCreated: firebase.database.ServerValue.TIMESTAMP,
      messageType: MessageType.Message
    };

    newMessagePostRef.set(newMessagePost
      ,(error) => {
        if (error) {
          thunkApi.dispatch(messageError({ id: newMessagePost.id }));
        } 
    });

    return newMessagePost;
});

export const sendHistory = createAsyncThunk('chat/sendHistory', (text, thunkApi) => {

    const myUser = thunkApi.getState().login.myUser;
    const newMessagePostRef = firebase.database().ref(`${userMessagesPath}/${myUser.id}`).push();
    const newActivityPostRef = firebase.database().ref(friendActivityPath).push();

    const newMessagePost = {
      userId: myUser.id,
      userName: myUser.name,
      text: text,
      id: newMessagePostRef.key,
      timeStampCreated: firebase.database.ServerValue.TIMESTAMP,
      messageType: MessageType.History
    };
    
    const newActivityPost = {
        ...newMessagePost,
        id: newActivityPostRef.key,
        messageType: MessageType.Activity
    }

    newMessagePostRef.set(newMessagePost);
    newActivityPostRef.set(newActivityPost);
    
    // It will be added to UI automatically if firebase successful saves it
    // as it will trigger listener to add as new message.
});

export const chatSlice = createSlice({
    name: 'chat',
    initialState: {
        messageIds: [],
        messages:{}
    },
    reducers: {
        messageReceived: (state, action) => {
            const message = action.payload;
            const messageExists = message.id in state.messages;
            message.status = ChatStatusType.SENT;

            if(!messageExists){
                state.messageIds.push(action.payload.id);
            }

            state.messages[message.id] = message;
        },
        messageError: (state, action) => {
            const message = action.payload;
            const messageExists = message.id in state.messages;

            if(messageExists){
                message.status = ChatStatusType.ERROR;
            }
        },
        clearMessages: (state, action) => {
            state.messages.length = 0;
            state.messageIds.length = 0;
        }
    },
    extraReducers: {
        [sendMessage.fulfilled]: (state, action) => {
            const message = action.payload;
            const messageExists = message.id in state.messages;

            // Possible server already saves the message and callbacked to save into store already.
            if(messageExists) return;
            
            state.messageIds.push(action.payload.id);
            state.messages[action.payload.id] = {
                ...action.payload, 
                timeStampCreated: (new Date()).getTime(),
                status: ChatStatusType.PENDING
            };
        }
    } 
});

export const { addMessage, clearMessages, messageReceived, messageError } = chatSlice.actions;

export const selectMessages = state => {
    return state.chat.messageIds.map( id => state.chat.messages[id]);
}

export default chatSlice.reducer;

