import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';

import axios from 'axios';

export const getLeaderBoardData = createAsyncThunk('leaderboard/get', async () => {

    const limit = process.env.REACT_APP_API_LEADERBOARD_LIMIT;

    const response = await axios.get(`${process.env.REACT_APP_API_URL}/leaderboard?limit=${limit}`);
    return response.data;
});

export const leaderBoardSlice = createSlice({
  name: 'leaderBoard',
  initialState: {
    mostBullish: null,
    mostBearish: null,
    bestPerformers: null,
    worstPerformers: null
  },
  reducers: {
  },
  extraReducers: {
    [getLeaderBoardData.pending]: (state, action) => {
      state.status = 'loading';
    },
    [getLeaderBoardData.fulfilled]: (state, action) => {
      Object.assign(state, action.payload ?? {});
      state.status = 'succeeded';
    },
    [getLeaderBoardData.rejected]: (state, action) => {
      state.status = 'failed';
    }
  }
});

export const selectLeaderBoard = function(state){
  return state.leaderBoard;
}

export default leaderBoardSlice.reducer;
