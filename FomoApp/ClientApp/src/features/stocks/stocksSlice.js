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
    
    const response = await axios.get(`${process.env.REACT_APP_API_URL}/votes?${idQuery}`);
    return response.data;
});


export const sendVote = createAsyncThunk('vote/sendVote', async (vote, thunkApi) => {
    thunkApi.dispatch(updateVote(vote));
    const response = await axios.post(`${process.env.REACT_APP_API_URL}/votes`, { symbolId: vote.symbolId, direction: vote.direction, delta: vote.delta});
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
        updateVote: (state, action) => {
            const vote = action.payload;

            if(!vote){
                return;
            }

            const symbolId = vote.symbolId;
            const votes = state.votes[symbolId];

            if(votes){
                votes.count += vote.delta;
                votes.myVoteDirection = vote.direction;
            }
            else{
                state.votes[symbolId] = {
                    symbolId: symbolId,
                    count: vote.delta,
                    myVoteDirection: vote.direction
                }
            }
        }
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
        },
        [fetchVoteData.fulfilled]: (state, action) => {
            if(action.payload){
                state.votes = action.payload;       
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

    if(portfolioSymbol.symbolId in state.stocks.singleQuoteData && state.stocks.singleQuoteData[portfolioSymbol.symbolId]){
        return state.stocks.singleQuoteData[portfolioSymbol.symbolId];
    }
    else{
        return {
            symbolId: portfolioSymbol.symbolId,
            ticker:  portfolioSymbol.ticker,
            price: "Pending",
            averagePrice: "--",
            change: "--",
        }
    }
}

export const selectVoteData = function(state, symbolId){

    if(symbolId in state.stocks.votes && state.stocks.votes[symbolId]){
        return state.stocks.votes[symbolId];
    }
    else{
        return {
            count: 0,
            myVote: 0,
            symbolId: symbolId
        };
    }
}

export const { updateVote } = stocksSlice.actions;


export default stocksSlice.reducer;
