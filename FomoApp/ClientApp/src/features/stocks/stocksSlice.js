import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';

import QueryHelper from '../../app/QueryHelper';
import axios from 'axios';
import { sendHistory } from '../chatbox/ChatSlice'

export const fetchStockSingleQuoteDatas = createAsyncThunk('stock/fetchStockSingleQuoteDatas', async (symbolIds, thunkApi) => {

    let idQuery = QueryHelper.createIdsQuery("sids", symbolIds);
    
    const response = await axios.get(`${process.env.REACT_APP_API_URL}/singleQuoteData?${idQuery}`);
    return response.data;

});

export const fetchVoteData = createAsyncThunk('vote/fetchVoteData', async (symbolIds, thunkApi) => {

    let idQuery = QueryHelper.createIdsQuery("sids", symbolIds);
    
    const response = await axios.get(`${process.env.REACT_APP_API_URL}/votes?${idQuery}`);
    return response.data;
});


export const sendVote = createAsyncThunk('vote/sendVote', async (vote, thunkApi) => {

    thunkApi.dispatch(updateVote(vote));
    const response = await axios.post(`${process.env.REACT_APP_API_URL}/votes`, { symbolId: vote.symbolId, direction: vote.direction, delta: vote.delta});
    
    const voteText = `${getVoteDirectionName(vote.direction)} ${vote.ticker}`;
     
    thunkApi.dispatch(sendHistory(voteText))
    return response.data;
});

function getVoteDirectionName(direction){
    switch(direction){
        case 1: return "Upvoted";
        case 0: return "Abstained";
        case -1: return "Downvoted";
        default: throw new Error("Unknown Vote direction");
    }
}

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
                state.votes = {...state.votes, ...action.payload};       
            }
        }
    }
});

export const selectStockLastUpdated = function(state, symbolId){
    if(state.stocks.singleQuoteData == null || !state.stocks.singleQuoteData[symbolId]){
        return new Date("2000-01-01");
    }

    return state.stocks.singleQuoteData[symbolId].lastUpdated;
}

export const selectStockData = function(state, portfolioSymbol){

    if(portfolioSymbol.symbolId in state.stocks.singleQuoteData && state.stocks.singleQuoteData[portfolioSymbol.symbolId]){
        return state.stocks.singleQuoteData[portfolioSymbol.symbolId];
    }
    else{
        return {
            symbolId: portfolioSymbol.symbolId,
            ticker:  portfolioSymbol.ticker,
            price: 0,
            change: 0,
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

export const VoteDirectionType = Object.freeze({ UPVOTE: 1, NONE: 0, DOWNVOTE: -1});

export const { updateVote } = stocksSlice.actions;


export default stocksSlice.reducer;
