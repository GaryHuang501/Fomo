import { fireEvent, screen, waitFor } from '@testing-library/react';

import EditPortfolioForm from './EditPortfolioForm';
import MockAdapter from 'axios-mock-adapter';
import React from 'react';
import { act } from 'react-dom/test-utils';
import axios from 'axios';
import { render } from '../../test-util';

const testUrl = "http://localhost";
let portfolioId;
let portfolioSymbol;
let initialState
let mock;

beforeEach(() => {
  mock = new MockAdapter(axios);

  process.env = {
      REACT_APP_API_URL: testUrl
  };

  portfolioId = 1000;

  portfolioSymbol = { id: 1, symbolId: 1, ticker: 'abc', averagePrice: 1.00, return: 1 };

  initialState = {
    portfolio:{
      ids: [portfolioId],
      selectedPortfolioId: portfolioId,
      portfolios: {
        [portfolioId]: {
          id: portfolioId,
          portfolioSymbols: [portfolioSymbol]
        }
      }
    },
    login: {
      selectedUser: {
          id: "200",
          name: "myUser"
      },
      myUser: {
          id: "200",
          name: "myUser"
      }
    } 
  };
});

afterEach(() => {
  mock.restore();
  process.env = {};
});

describe("client input error when invalid value", () => {

  const cases = [[-2.5, 'Value must be positive number.'], [20000000, 'Max value is 10000000'] ];

  test.each(cases)(
    "Expect value %d to return error %s",
    async (testValue, expectedError) => {
      
      let submitCalled = false;

      function onSubmit(){
          submitCalled = true;
      }
    
      act(() => {
        render(<EditPortfolioForm onSubmit={onSubmit} portfolioSymbolId={portfolioSymbol.id}/>, { initialState });
      });
    
      const averagePriceInput = screen.getByRole('spinbutton');
      const submit = screen.getByRole('button');
    
      fireEvent.change(averagePriceInput, { target: { value: testValue}} );
      fireEvent.click(submit);
    
      await waitFor( () => expect(screen.getByText(expectedError)));
    
      expect(submitCalled).toBe(false);
    }
  );
});

it("updates average price when submitted", async () => {

    let submitCalled = false;

    function onSubmit(){
        submitCalled = true;
    }
    
    const endPointUrl = `${process.env.REACT_APP_API_URL}/portfolios/${portfolioId}/portfolioSymbols/${portfolioSymbol.id}`

    const spy = jest.spyOn(axios, 'patch');

    act(() => {
      render(<EditPortfolioForm onSubmit={onSubmit} portfolioSymbolId={portfolioSymbol.id}/>, { initialState });
    });

    const averagePriceInput = screen.getByRole('spinbutton');
    const submit = screen.getByRole('button');

    fireEvent.change(averagePriceInput, { target: { value: 2.50 }} );
    fireEvent.click(submit);

    await waitFor( () => expect(spy).toHaveBeenCalledWith(endPointUrl, [{ op: "replace", path: "/averagePrice", value: "2.50"}]));

    expect(submitCalled).toBe(true);

  });
