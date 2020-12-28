import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import axios from 'axios';

const apiUrl = process.env.REACT_APP_API_URL;
const searchLimit = process.env.REACT_APP_STOCK_SEARCH_RESULTS_LIMIT;

export const searchStocks = createAsyncThunk('stockSearch/fetchStocks', async (keywords, thunkApi) => {
        const { stockSearch } = thunkApi.getState();

        if(keywords in stockSearch.stocks){
            return stockSearch.stocks[keywords];
        }

        const response = await axios.get(`${apiUrl}/symbols/?keywords=${keywords}&limit=${searchLimit}`)
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
      state.status = 'succeeded';      
      state.stocks[action.meta.arg] = action.payload ?? [];
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
