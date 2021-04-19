import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';

import axios from 'axios';

export const searchStocks = createAsyncThunk('stockSearch/fetchStocks', async (keywords, thunkApi) => {
        const { stockSearch } = thunkApi.getState();

        if(keywords in stockSearch.stocks){
            return stockSearch.stocks[keywords];
        }

        const response = await axios.get(`${process.env.REACT_APP_API_URL}/symbols`, { params : { keywords: keywords, limit: process.env.REACT_APP_STOCK_SEARCH_RESULTS_LIMIT }});
        return response.data;
}, {
    condition: (keywords) => {
        if(!keywords || keywords.trim().length === 0){
            return false;
        }
    }
});

export const stockSearchSlice = createSlice({
  name: 'stockSearch',
  initialState: {
      stocks:{      
      },
      status: 'idle'
  },
  reducers: {
  },
  extraReducers: {
    [searchStocks.pending]: (state, action) => {
      state.status = 'loading';
    },
    [searchStocks.fulfilled]: (state, action) => {
      state.stocks[action.meta.arg] = action.payload ?? [];
      state.status = 'succeeded';      
    },
    [searchStocks.rejected]: (state, action) => {
      state.status = 'failed';
    }
  }
});

export const selectStockSearchResults = function(state, keywords){
  if(keywords in state.stockSearch.stocks && Array.isArray(state.stockSearch.stocks[keywords])){
    return state.stockSearch.stocks[keywords];
  }

  return [];
}

export const selectStockSearchStatus = state => state.stockSearch.status;

export default stockSearchSlice.reducer;
