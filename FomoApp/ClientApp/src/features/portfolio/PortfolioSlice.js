import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';

import axios from 'axios';

const apiUrl = process.env.REACT_APP_API_URL;

export const fetchPortfolioIds = createAsyncThunk('portfolio/fetchPortfoliIds', async (thunkApi) => {
    const response = await axios.get(`${apiUrl}/portfolios`);

    return response.data;
});

export const fetchPortfolio = createAsyncThunk('portfolio/fetchPortfolio', async (id, thunkApi) => {
    const response = await axios.get(`${apiUrl}/portfolios/${id}`);

    return response.data;
});

export const addPortfolioStock = createAsyncThunk('portfolio/addPortfolioStock', async (payload, thunkApi) => {
    const response = await axios.post(`${apiUrl}/portfolios/${payload.portfolioId}/portfolioSymbols`, {symbolId: payload.symbolId});

    return response.data;
});

export const portfolioSlice = createSlice({
    name: 'portfolio',
    initialState: {
        ids: [],
        selectedPortfolioId: null,
        portfolios: {

        },
        fetchIdStatus: "idle",
        fetchPortfolioStatus: "idle",
        addPortfolioSymbolStatus: "idle"
    },
    reducers: {
        setSelectedPortfolioId: (state, action) => {
            state.selectedPortfolioId = action.payload;
        }
    },
    extraReducers: {
        [fetchPortfolioIds.pending]: (state, action) => {
            state.fetchIdStatus = 'loading';
        },
        [fetchPortfolioIds.fulfilled]: (state, action) => {
            state.fetchIdStatus = 'succeeded';
            state.ids = action.payload ?? [];
        },
        [fetchPortfolioIds.rejected]: (state, action) => {
            state.fetchIdStatus = 'failed';
        },
        [fetchPortfolio.pending]: (state, action) => {
            state.fetchPortfolioStatus = 'loading';
        },
        [fetchPortfolio.fulfilled]: (state, action) => {
            state.fetchPortfolioStatus = 'succeeded';
            state.portfolios[action.meta.arg] = action.payload;
        },
        [fetchPortfolio.rejected]: (state, action) => {
            state.fetchPortfolioStatus = 'failed';
        },
        [addPortfolioStock.pending]: (state, action) => {
            state.addPortfolioSymbolStatus = 'loading';
        },
        [addPortfolioStock.fulfilled]: (state, action) => {
            state.addPortfolioSymbolStatus = 'succeeded';
            state.portfolios[state.selectedPortfolioId].portfolioSymbols.push(action.payload);

        },
        [addPortfolioStock.rejected]: (state, action) => {
            state.addPortfolioSymbolStatus = 'rejected';
        },
    } 
});

export const { setSelectedPortfolioId } = portfolioSlice.actions;

export const selectPortfolioIds = state => state.portfolio.ids;

export const selectSelectedPortfolioId = state => state.portfolio.selectedPortfolioId;

export const selectPortfolio = function(state){  
    const portfolio = state.portfolio.portfolios[state.portfolio.selectedPortfolioId];
    return portfolio ? portfolio : { portfolioSymbols: [] };
}

export default portfolioSlice.reducer;
