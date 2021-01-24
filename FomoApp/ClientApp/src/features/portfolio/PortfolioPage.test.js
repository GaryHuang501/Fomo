import { fireEvent, screen, waitFor, within } from '@testing-library/react';

import MockAdapter from 'axios-mock-adapter';
import { PortfolioPage } from './PortfolioPage';
import React from 'react';
import { act } from 'react-dom/test-utils';
import axios from 'axios';
import { render } from '../../test-util';

beforeEach(() => {
});

afterEach(() => {
});

it("renders portfolio", async () => {

    const mock = new MockAdapter(axios);
    const portfolioId = 1;

    mock.onGet(`${process.env.REACT_APP_API_URL}/portfolios`)
        .reply(200, [portfolioId]);

    act(() => {
        render(<PortfolioPage />);
    });

    await waitFor(() => expect(screen.getByRole("table")).toBeInTheDocument());
});
