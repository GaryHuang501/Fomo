import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';

import axios from 'axios';

export const getClientCustomToken = createAsyncThunk('login/firebase-authenticate', async () => {
    const response = await axios.get(`${process.env.REACT_APP_API_URL}/accounts/ClientCustomToken`);
    return response.data;

});

export const getAccount = createAsyncThunk('accounts/getAccount', async () => {
    const response = await axios.get(`${process.env.REACT_APP_API_URL}/accounts`)
    return response.data;
});

export const getAccountForId = createAsyncThunk('accounts/getAccount/id', async (id) => {
    const response = await axios.get(`${process.env.REACT_APP_API_URL}/accounts/${id}`)
    return response.data;
});

export const updateAccount = createAsyncThunk('accounts/updateAccount/id', async (account, thunkApi) => {

    try{
        const response = await axios.put(`${process.env.REACT_APP_API_URL}/accounts/${account.id}`, account)

        return response.data;

    }catch(error){
        return thunkApi.rejectWithValue(error.response.data);
    }
});


function revokeCredentials(state){
    state.isAuthenticated = false;
    state.isFirebaseAuthenticated = false;
    state.customClientToken = false;
    state.selectedUser = null;
    state.myUser = null;
}

export const loginSlice = createSlice({
  name: 'login',
  initialState: {
    isAuthenticated: false, 
    isFirebaseAuthenticated: false,
    customClientToken: null,
    selectedUser: null,
    myUser: null,
    updateError: null,
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
        state.myUser = action.payload;
    },
    [getAccount.rejected]: (state, action) => {
        state.status = 'failed'
        state.isAuthenticated = false;
    },
    [getAccountForId.pending]: (state, action) => {
        state.status = 'loading'
    },
    [getAccountForId.fulfilled]: (state, action) => {
        state.status = 'succeeded'
        state.selectedUser = action.payload;
    },
    [getAccountForId.rejected]: (state, action) => {
        state.status = 'failed'
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
    },
    [updateAccount.fulfilled]: (state, action) => {
        state.updateStatus = 'succeeded'
        state.updateError = null;

        if(state.myUser.id === state.selectedUser.id){
            state.selectedUser = action.payload;
        }

        state.myUser = action.payload;
    },
    [updateAccount.pending]: (state, action) => {
        state.updateStatus = 'loading';
        state.updateError = null;
    },
    [updateAccount.rejected]: (state, action) => {
        state.updateStatus = 'failed'
        state.updateError = action.payload;
    }
  }
});

export const { setAuthenticated, setUnauthenticated, setFireBaseAuthenticated } = loginSlice.actions;

export const selectAuthenticatedState = state => state.login.isAuthenticated;

export const selectClientCustomToken = state => state.login.customClientToken;

export const selectFirebaseAuthenticatedState = state => state.login.isFirebaseAuthenticated;

export const selectUser = state => state.login.selectedUser;

export const selectMyUser = state => state.login.myUser;

export const selectIsMyUserPage = state => (state.login.selectedUser && state.login.myUser) && (state.login.selectedUser.id === state.login.myUser.id);

export const selectUpdateStatus = state => state.login.updateStatus;

export const selectUpdateError = state => state.login.updateError;

export default loginSlice.reducer;
