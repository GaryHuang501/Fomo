import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';

import axios from 'axios';

const apiUrl = process.env.REACT_APP_API_URL;

export const fetchStockSingleQuote = createAsyncThunk('stocks/fetchStockSingleQuote', async (symbolId, thunkApi) => {
    const response = await axios.get(`${apiUrl}/stocks/singlequote/${symbolId}`);

    return response.data;
});

export const stocksSlice = createSlice({
    name: 'stocks',
    initialState: {
        singleQuote:{      
        }
    },
    reducers: {
        updateSingleQuote: (state, action) => {
            state.singleQuote[action.payload.symbolId] = action.payload;
        }
    },
    extraReducers: {
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
