import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import axios from 'axios';
import config from '../../app/Config.json'

export const searchStocks = createAsyncThunk('stockSearch/fetchStocks', async (keywords, thunkApi) => {
        const { stockSearch } = thunkApi.getState();

        if(keywords in stockSearch.stocks){
            return stockSearch.stocks[keywords];
        }

        const response = await axios.get(`${config.apiUrl}/symbols/?keywords=${keywords}&limit=${config.stockSearchResultsLimit}`)
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
      }
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

export const selectStockSearchResults = state => state.stockSearch.stocks;

export const selectStockSearchStatus = state => state.stockSearch.status;

export default stockSearchSlice.reducer;
