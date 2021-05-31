import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';
import { friendActivityPath, userMessagesPath } from '../../app/FireBasePaths';

import ChatStatusType from './ChatStatusType';
import MessageType  from './MessageType';
import firebase from 'firebase/app';

export const sendMessage = createAsyncThunk('chat/sendMessage', (messagePayload, thunkApi) => {

    const message = messagePayload.message;
    const path = messagePayload.path;

    const newMessagePostRef = firebase.database().ref(path).push();

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
          thunkApi.dispatch(messageError({ id: newMessagePost.id, path: path }));
        } 
    });

    return {path: path, message: newMessagePost};
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

function AddMessageToState(state, message, path, status){
    const pathExists = path in state;

    if(!pathExists){
        state[path] = {
            messageIds: [],
            messages:{}
        }
    }

    const messageExists = message.id in state[path].messages;
    message.status = status;

    if(!messageExists){
        state[path].messageIds.push(message.id);
    }

    state[path].messages[message.id] = message;
}

export const chatSlice = createSlice({
    name: 'chat',
    initialState: {
    },
    reducers: {
        messageReceived: (state, action) => {
            const path = action.payload.path;
            const message = action.payload.message;

            AddMessageToState(state, message, path, ChatStatusType.SENT);
        },
        messageError: (state, action) => {
            const path = action.payload.path;
            const messageId = action.payload.id;
            const messageExists = path in state && messageId in state[path].messages;

            if(messageExists){
                state[path].messages[messageId] = ChatStatusType.ERROR;
            }
        },
    },
    extraReducers: {
        [sendMessage.fulfilled]: (state, action) => {
            const message = action.payload.message;
            const path = action.payload.path;
            const messageExists = path in state && message.id in state[path].messages;

            // Possible server already saves the message and callbacked to save into store already.
            if(messageExists) return;

            message.timeStampCreated = (new Date()).getTime();

            AddMessageToState(state, message, path, ChatStatusType.PENDING);          
        }
    } 
});

export const { addMessage, clearMessages, messageReceived, messageError } = chatSlice.actions;

export function selectMessages(state, path){
    if(path in state.chat){
        const chat = state.chat[path];
        return chat.messageIds.map( id => chat.messages[id]);
    }
    else{
        return [];
    }
}

export function selectRefAlreadyExists(state, path){
    return path in state.chat;
}

export default chatSlice.reducer;

