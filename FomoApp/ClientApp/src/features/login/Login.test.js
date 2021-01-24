import 'firebase/auth';
import 'firebase/database';
import 'firebase/analytics';

import { fireEvent, screen, waitFor, within } from '@testing-library/react';

import App  from '../../App';
import MockAdapter from 'axios-mock-adapter';
import React from 'react';
import { act } from 'react-dom/test-utils';
import axios from 'axios';
import firebase from 'firebase/app';
import { render } from '../../test-util';

beforeEach(() => {
    
    firebase.initializeApp = jest.fn();
    firebase.analytics = jest.fn();

    process.env = {
        REACT_APP_API_URL: "https://test.com",
    };
});

afterEach(() => {
    process.env = {};
    firebase.initializeApp.mockClear();
    firebase.analytics.mockClear();
});

it("Should render login modal when unauthenticated request made", async () => {

    const mock = new MockAdapter(axios);

    mock.onGet(`${process.env.REACT_APP_API_URL}/accounts/checklogin`)
        .reply(401, {});

    act(() => {
        render(<App/>);
    });

    await waitFor(() => expect(screen.getByText('Please Select a Login')).toBeInTheDocument());
});

it("Should not render login modal when authenticated request made", async () => {
    const mock = new MockAdapter(axios);

    mock.onGet(`${process.env.REACT_APP_API_URL}/accounts/checklogin`)
        .reply(200, {});

    act(() => {
        render(<App/>);
    });

    expect(screen.queryByText('Please Select a Login')).not.toBeInTheDocument()
});
