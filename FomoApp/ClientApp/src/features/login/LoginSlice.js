import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';

import axios from 'axios';

export const authenticate = createAsyncThunk('login/authenticate', async () => {
    await axios.get(`${process.env.REACT_APP_API_URL}/accounts/authenticate`);
});

export const getClientCustomToken = createAsyncThunk('login/firebase-authenticate', async () => {
    const response = await axios.get(`${process.env.REACT_APP_API_URL}/accounts/ClientCustomToken`);
    return response.data;

});

export const checkLogin = createAsyncThunk('login/checkLogin', async () => {
    await axios.get(`${process.env.REACT_APP_API_URL}/accounts/checklogin`)
});

export const loginSlice = createSlice({
  name: 'login',
  initialState: {
    isAuthenticated: false, 
    isFirebaseAuthenticated: false,
    customClientToken: null
  },
  reducers: {
      setAuthenticated: state => {
          state.isAuthenticated = true;
      },
      setUnauthenticated: state => {
          state.isAuthenticated = false;
      },
      setFireBaseAuthenticated: state => {
          state.isFirebaseAuthenticated = true;
      }
  },
  extraReducers: {
    [authenticate.pending]: (state, action) => {
      state.status = 'loading'
    },
    [authenticate.fulfilled]: (state, action) => {
      state.status = 'succeeded'
      state.isAuthenticated = true;
    },
    [authenticate.rejected]: (state, action) => {
      state.status = 'failed'
      state.isAuthenticated = false;
    },
    [checkLogin.pending]: (state, action) => {
        state.status = 'loading'
    },
    [checkLogin.fulfilled]: (state, action) => {
        state.status = 'succeeded'
        state.isAuthenticated = true;
    },
    [checkLogin.rejected]: (state, action) => {
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


export default loginSlice.reducer;
