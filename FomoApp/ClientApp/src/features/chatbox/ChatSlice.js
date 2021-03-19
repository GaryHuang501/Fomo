import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';

import ChatStatusType from './ChatStatusType';
import firebase from 'firebase/app';
import { userMessagesPath } from '../../app/FireBasePaths';

export const sendMessage = createAsyncThunk('chat/sendMessage', (messagePayLoad, thunkApi) => {

    const newMessagePostRef = firebase.database().ref(`${userMessagesPath}/${messagePayLoad.userId}`).push();

    const newMessagePost = {
      ...messagePayLoad,
      text: messagePayLoad.text,
      id: newMessagePostRef.key,
      timeStampCreated: firebase.database.ServerValue.TIMESTAMP,
    };

    newMessagePostRef.set(newMessagePost
      ,(error) => {
        if (error) {
          thunkApi.dispatch(messageError({ id: newMessagePost.id }));
        } 
    });

    return newMessagePost;
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
            state.messages = [];
            state.messageIds = [];
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

