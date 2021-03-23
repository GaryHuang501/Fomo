import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';

import QueryHelper from '../../app/QueryHelper';
import axios from 'axios';

export const fetchStockSingleQuoteDatas = createAsyncThunk('stock/fetchStockSingleQuoteDatas/', async (symbolIds, thunkApi) => {

    let idQuery = QueryHelper.createIdsQuery("sids", symbolIds);
    
    const response = await axios.get(`${process.env.REACT_APP_API_URL}/singleQuoteData?${idQuery}`);
    return response.data;

});

export const fetchVoteData = createAsyncThunk('vote/fetchVoteData/', async (symbolIds, thunkApi) => {

    let idQuery = QueryHelper.createIdsQuery("sids", symbolIds);
    
    const response = await axios.get(`${process.env.REACT_APP_API_URL}/vote?${idQuery}`);
    return response.data;
});

export const sendVote = createAsyncThunk('vote/sendVote', async (vote) => {

    const response = await axios.post(`${process.env.REACT_APP_API_URL}/vote`, { symbolId: vote.symbolId, direction: vote.dir});
    return response.data;
});

export const stocksSlice = createSlice({
    name: 'stocks',
    initialState: {
        singleQuoteData:{      
        },
        votes:{ 
        }
    },
    reducers: {
    },
    extraReducers: {
        [fetchStockSingleQuoteDatas.fulfilled]: (state, action) => {
            for(const newSingleQuoteData of action.payload){

                const current = state.singleQuoteData[action.payload.symbolId];
                
                const noCurrentDataExists = !current;
                const isStaleData = noCurrentDataExists || Date.parse(newSingleQuoteData.lastUpdated) > Date.parse(current.lastUpdated);

                if(isStaleData){                  
                    state.singleQuoteData[newSingleQuoteData.symbolId] = newSingleQuoteData.data;
                } 
            }
        }
    }
});

export const selectStocksLastUpdatedDates = function(state){

    const dates = {};

    for(const symbolId in state.stocks.singleQuoteData){

        if(state.stocks.singleQuoteData == null || !state.stocks.singleQuoteData[symbolId]){
            dates[symbolId] = new Date("2000-01-01");
        }
        else
        {
            dates[symbolId] = state.stocks.singleQuoteData[symbolId].lastUpdated;
        }
    }

    return dates;
}


export const selectStockData = function(state, portfolioSymbol){

    if(portfolioSymbol.symbolId in state.stocks.singleQuoteData 
        && state.stocks.singleQuoteData[portfolioSymbol.symbolId]){
        return state.stocks.singleQuoteData[portfolioSymbol.symbolId];
    }
    else{
        return {
            symbolId: portfolioSymbol.symbolId,
            ticker:  portfolioSymbol.ticker,
            price: "Pending",
            averagePrice: "--",
            change: "--",
            votes: 0
        }
    }
}

export default stocksSlice.reducer;
