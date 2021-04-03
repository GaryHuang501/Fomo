import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';

import axios from 'axios';

export const getClientCustomToken = createAsyncThunk('login/firebase-authenticate', async () => {
    const response = await axios.get(`${process.env.REACT_APP_API_URL}/accounts/ClientCustomToken`);
    return response.data;

});

export const getAccount = createAsyncThunk('login/getAccount', async () => {
    const response = await axios.get(`${process.env.REACT_APP_API_URL}/accounts`)
    return response.data;
});

function revokeCredentials(state){
    state.isAuthenticated = false;
    state.isFirebaseAuthenticated = false;
    state.customClientToken = false;
    state.user = null;
}

export const loginSlice = createSlice({
  name: 'login',
  initialState: {
    isAuthenticated: false, 
    isFirebaseAuthenticated: false,
    customClientToken: null,
    user: null
  },
  reducers: {
      setAuthenticated: state => {
          state.isAuthenticated = true;
      },
      setUnauthenticated: state => {
        revokeCredentials(state);
      },
      setFireBaseAuthenticated: state => {
          state.isFirebaseAuthenticated = true;
      }
  },
  extraReducers: {
    [getAccount.pending]: (state, action) => {
        state.status = 'loading'
    },
    [getAccount.fulfilled]: (state, action) => {
        state.status = 'succeeded'
        state.isAuthenticated = true;
        state.user = action.payload;
    },
    [getAccount.rejected]: (state, action) => {
        state.status = 'failed'
        state.isAuthenticated = false;
    },
    [getClientCustomToken.pending]: (state, action) => {
      state.status = 'loading'
    },
    [getClientCustomToken.fulfilled]: (state, action) => {
        state.status = 'succeeded'
        state.customClientToken = action.payload;
    },
    [getClientCustomToken.rejected]: (state, action) => {
        state.status = 'failed'
        state.isFirebaseAuthenticated = false;
    }
  }
});

export const { setAuthenticated, setUnauthenticated, setFireBaseAuthenticated } = loginSlice.actions;

export const selectAuthenticatedState = state => state.login.isAuthenticated;

export const selectClientCustomToken = state => state.login.customClientToken;

export const selectFirebaseAuthenticatedState = state => state.login.isFirebaseAuthenticated;

export const selectUser = state => state.login.user;

export default loginSlice.reducer;
