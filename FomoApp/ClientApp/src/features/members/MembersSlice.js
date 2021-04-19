import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';

import axios from 'axios';

export const getMemberData = createAsyncThunk('members/getMemberData', async () => {

    // TODO: Pagination support
    const limit = process.env.REACT_APP_API_MEMBER_LIMIT;
    const offset = 0;

    const response = await axios.get(`${process.env.REACT_APP_API_URL}/members?limit=${limit}&offset=${offset}`);
    return response.data;
});

export const membersSlice = createSlice({
  name: 'members',
  initialState: {
      memberGroupings: {       
      },
      uncategorizedMembers: [],
      total: 0,
      offset: 0,
      limit: 0
  },
  reducers: {
  },
  extraReducers: {
    [getMemberData.pending]: (state, action) => {
      state.status = 'loading';
    },
    [getMemberData.fulfilled]: (state, action) => {
      Object.assign(state, action.payload ?? {});
      state.status = 'succeeded';      
    },
    [getMemberData.rejected]: (state, action) => {
      state.status = 'failed';
    }
  }
});

export const selectMemberData = function(state){
  return state.members;
}

export default membersSlice.reducer;
