import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';

import axios from 'axios';

const apiUrl = process.env.REACT_APP_API_URL;

export const authenticate = createAsyncThunk('login/authenticate', async () => {
    await axios.get(`${apiUrl}/accounts/authenticate`);
});

export const checkLogin = createAsyncThunk('login/checkLogin', async () => {
    await axios.get(`${apiUrl}/accounts/checklogin`)
});

export const loginSlice = createSlice({
  name: 'login',
  initialState: {
    isAuthenticated: true
  },
  reducers: {
      setAuthenticated: state => {
          state.isAuthenticated = true;
      },
      setUnauthenticated: state => {
          state.isAuthenticated = false;
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
    }
  }
});

export const { setAuthenticated, setUnauthenticated } = loginSlice.actions;

export const selectAuthenticatedState = state => state.login.isAuthenticated;

export default loginSlice.reducer;
