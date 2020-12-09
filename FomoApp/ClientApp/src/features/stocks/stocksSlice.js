import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import axios from 'axios';
import config from '../../app/Config.json'

export const fetchStockSingleQuote = createAsyncThunk('stocks/fetchStockSingleQuote', async (symbolId, thunkApi) => {
    const response = await axios.get(`${config.apiUrl}/stocks/singlequote/${symbolId}`);

    return response.data;
});

export const stocksSlice = createSlice({
    name: 'stocks',
    initialState: {
        singleQuote:{      
        }
    },
    reducers: {
    },
    extraReducers: {
        [fetchStockSingleQuote.pending]: (state, action) => {
            state.status = 'loading';
        },
        [fetchStockSingleQuote.fulfilled]: (state, action) => {
            state.status = 'succeeded';
            state.portfolios[action.meta.arg] = action.payload;
        },
        [fetchStockSingleQuote.rejected]: (state, action) => {
            state.status = 'failed';
        }
    }
});


export const selectStockData = function(state, symbol){
    if(symbol.id in state.stocks.singleQuote){
        return state.stocks.singleQuote[symbol.id];
    }
    else{
        return {
            symbolId: symbol.id,
            ticker:  symbol.ticker,
            marketPrice: "Pending",
            averagePrice: "--",
            change: "--",
            bull: "--",
            bear: "--"
        }
    }
}

export const selectSelectedPortfolioId = state => state.portfolio.selectedPortfolioId;


export default stocksSlice.reducer;
