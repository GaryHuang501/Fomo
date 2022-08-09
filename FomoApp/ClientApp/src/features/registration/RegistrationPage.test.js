import { fireEvent, screen, waitFor } from '@testing-library/react';

import MockAdapter from 'axios-mock-adapter';
import React from 'react';
import RegistrationPage from './RegistrationPage'
import { act } from 'react-dom/test-utils';
import axios from 'axios';
import { render } from '../../test-util';

const testUrl = "http://localhost";
let mock;
let mockUseParams = {};

jest.mock('react-router-dom', () => ({
    useParams: function(){
        return mockUseParams;
    }
}));

beforeEach(() => {
    mock = new MockAdapter(axios);

    process.env = {
        REACT_APP_API_URL: testUrl
    };
});

afterEach(() => {
    mock.restore();
    jest.clearAllMocks();
    process.env = {};
});

it("Should show api error when registration fails", async () => {

    mockUseParams = { urlUserId: 100 };
    const spy = jest.spyOn(axios, 'put');

    act(() => {
        render(<RegistrationPage/>);
    });

    const endPointUrl = `${process.env.REACT_APP_API_URL}/accounts/${mockUseParams.urlUserId}`
    mock.onPut(endPointUrl)
        .reply(404, "Failed");

    const inputs = screen.getAllByRole('textbox');
    const submit = screen.getByRole('button');

    const nameInput = inputs[0];

    fireEvent.change(nameInput, { target: { value: 'updatedName' } });
    fireEvent.click(submit);

    await waitFor(() => expect(screen.getByText("Failed")).toBeInTheDocument());
});


it("Should register new user", async () => {

    mockUseParams = { urlUserId: 100 };
    const spy = jest.spyOn(axios, 'put');

    act(() => {
        render(<RegistrationPage/>);
    });

    const endPointUrl = `${process.env.REACT_APP_API_URL}/accounts/${mockUseParams.urlUserId}`
    mock.onPut(endPointUrl)
        .reply(200);

    const inputs = screen.getAllByRole('textbox');
    const submit = screen.getByRole('button');

    const nameInput = inputs[0];

    fireEvent.change(nameInput, { target: { value: 'updatedName' } });
    fireEvent.click(submit);

    await waitFor(() => expect(spy).toHaveBeenCalledWith(endPointUrl, { id: 100, name: 'updatedName' },  {"withCredentials": true}));
});
