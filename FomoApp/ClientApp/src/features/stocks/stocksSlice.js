import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';

import axios from 'axios';

const apiUrl = process.env.REACT_APP_API_URL;

export const fetchStockSingleQuoteDatas = createAsyncThunk('fetchStockSingleQuoteDatas/', async (symbolIds, thunkApi) => {

    let idQuery = "";

    for(let i = 0; i < symbolIds.length; i++){
        const id = symbolIds[i];

        if(i !== 0){
            idQuery += "&"
        }

        idQuery += `symbolIds=${id}`
    }

    console.log(`${apiUrl}/singleQuoteData?${idQuery}`);
    const response = await axios.get(`${apiUrl}/singleQuoteData?${idQuery}`);

    return response.data;
});

export const stocksSlice = createSlice({
    name: 'stocks',
    initialState: {
        singleQuoteData:{      
        }
    },
    reducers: {
    },
    extraReducers: {
        [fetchStockSingleQuoteDatas.fulfilled]: (state, action) => {
            console.log(action);
            for(const newSingleQuoteData of action.payload){

                const current = state.singleQuoteData[action.payload.symbolId];
                
                const noCurrentDataExists = !current;
                const isStaleData = noCurrentDataExists || Date.parse(newSingleQuoteData.lastUpdated) > Date.parse(current.lastUpdated);

                if(isStaleData){ 
                    
                    const noDataOnServerYet = newSingleQuoteData.data === null;

                    if (noDataOnServerYet){
                        state.singleQuoteData[newSingleQuoteData.symbolId] = null
                    }
                    else{
                        state.singleQuoteData[newSingleQuoteData.symbolId] = newSingleQuoteData.data;
                    }
                } 
            }
        },
    }
});

export const selectStocksLastUpdatedDates = function(state){

    const dates = {};

    for(const symbolId in state.singleQuoteData){

        if(state.singleQuoteData == null){
            dates[symbolId] = new Date("2000-01-01");
        }
        else
        {
            dates[symbolId] = state.singleQuoteData[symbolId].lastUpdated;
        }
    }

    return dates;
}


export const selectStockData = function(state, portfolioSymbol){
    if(portfolioSymbol.symbolId in state.stocks.singleQuoteData){
        return state.stocks.singleQuoteData[portfolioSymbol.symbolId];
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
