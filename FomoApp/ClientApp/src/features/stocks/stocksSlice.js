import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';

import axios from 'axios';

const apiUrl = process.env.REACT_APP_API_URL;

export const fetchStockSingleQuotes = createAsyncThunk('fetchStockSingleQuote/', async (symbolIds, thunkApi) => {

    let idQuery = "";

    for(let i = 0; i < symbolIds.length; i++){
        const id = symbolIds[i];

        if(i !== 0){
            idQuery += "&"
        }

        idQuery += `symbolIds=${id}`
    }

    console.log(`${apiUrl}/SingleQuoteData?${idQuery}`);
    const response = await axios.get(`${apiUrl}/SingleQuoteData?${idQuery}`);

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
        [fetchStockSingleQuotes.fulfilled]: (state, action) => {
            console.log(action);
            return;
            for(const newSingleQuote of action.payload){
                const current = state.singleQuote[action.payload.symbolId];

                if(!current && ( Date.parse(newSingleQuote.Data.lastUpdated) > Date.parse(current.lastUpdated)) ){
                    state.singleQuote[current.symbolId] = newSingleQuote.data;
                } 
            }
        },
    }
});


export const selectStockData = function(state, portfolioSymbol){
    if(portfolioSymbol.symbolId in state.stocks.singleQuote){
        return state.stocks.singleQuote[portfolioSymbol.symbolId];
    }
    else{
        return {
            symbolId: portfolioSymbol.symbolId,
            ticker:  portfolioSymbol.ticker,
            marketPrice: "Pending",
            averagePrice: "--",
            change: "--",
            bull: "--",
            bear: "--"
        }
    }
}

export default stocksSlice.reducer;
