import { fireEvent, screen, waitFor } from '@testing-library/react';
import { selectMyUser, selectUser } from '../../features/login/LoginSlice';

import MockAdapter from 'axios-mock-adapter';
import ProfileModal from './ProfileModal';
import React from 'react';
import { act } from 'react-dom/test-utils';
import axios from 'axios';
import { modal } from './Modal';
import { render } from '../../test-util';
import { useSelector } from 'react-redux';

const testUrl = "http://localhost";
let mock;

jest.mock('./Modal', () => (props) => (<div>{props.children}</div>));

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

function TestMyUserSettingsComponent() {
    const user = useSelector(selectMyUser);

    return (
        <div>
            <p>{user.name}</p>
        </div>)
}

function TestSelectedUserSettingsComponent() {

    const user = useSelector(selectUser);
    
    return (
        <div>
            <p>{user.name}</p>
        </div>)
}

it("Should not show modal when showProfile is false", () => {

    const initialState = {
        login: {
            myUser: {
                id: "200",
                name: "myUser"
            }
        }
    };

    const myUser = initialState.login.myUser;

    act(() => {
        render(<ProfileModal/>, { initialState });
    });
    
    expect(screen.queryAllByRole("textbox").length).toBe(0);
});

it("Should show modal when showProfile is true", async () => {

    const initialState = {
        login: {
            myUser: {
                id: "200",
                name: "myUser"
            }
        },
        modal: {
            showProfileModal: true
        }
    };

    const myUser = initialState.login.myUser;

    act(() => {
        render(<ProfileModal/>, { initialState });
    });

    await waitFor(() => expect(screen.getByRole("textbox")).toBeInTheDocument()); 
});

it("Should update my user profile when submitting", async () => {

    const initialState = {
        login: {
            selectedUser: {
                id: "200",
                name: "myUser"
            },
            myUser: {
                id: "200",
                name: "myUser"
            }
        },
        modal: {
            showProfileModal: true
        }
    };

    const myUser = initialState.login.myUser;

    const spy = jest.spyOn(axios, 'put');

    const endPointUrl = `${process.env.REACT_APP_API_URL}/accounts/${myUser.id}`
    mock.onPut(endPointUrl)
        .reply(200, { ...myUser, name: 'updatedName' });

    act(() => {
        render(<div><TestMyUserSettingsComponent /><ProfileModal /></div>, { initialState });
    });

    const inputs = screen.getAllByRole('textbox');
    const submit = screen.getByRole('button');

    const nameInput = inputs[0];

    fireEvent.change(nameInput, { target: { value: 'updatedName' } });
    fireEvent.click(submit);

    await waitFor(() => expect(spy).toHaveBeenCalledWith(endPointUrl, { ...myUser, name: 'updatedName' }));

    await waitFor(() => {
        expect(screen.getByText('updatedName'));
    });
});

it("Should update not update selected user when not same as my user during submission", async () => {


    const initialState = {
        login: {
            selectedUser: {
                id: "200",
                name: "myUser"
            },
            myUser: {
                id: "200",
                name: "myUser"
            }
        },
        modal: {
            showProfileModal: true
        }
    };

    const myUser = initialState.login.myUser;
    const spy = jest.spyOn(axios, 'put');

    act(() => {
        render(<div><TestSelectedUserSettingsComponent /><ProfileModal myUser={myUser}/></div>, { initialState });
    });

    const endPointUrl = `${process.env.REACT_APP_API_URL}/accounts/${myUser.id}`
    mock.onPut(endPointUrl)
        .reply(200, { ...myUser, name: 'updatedName' });

    const inputs = screen.getAllByRole('textbox');
    const submit = screen.getByRole('button');

    const nameInput = inputs[0];

    fireEvent.change(nameInput, { target: { value: 'updatedName' } });
    fireEvent.click(submit);

    await waitFor(() => expect(spy).toHaveBeenCalledWith(endPointUrl, { ...myUser, name: 'updatedName' }));

    await waitFor(() => {
        expect(screen.queryByText('updatedName')).not.toBeInTheDocument();
    });

});

it("Should show api error when update fails", async () => {


    const initialState = {
        login: {
            myUser: {
                id: "200",
                name: "myUser"
            }
        },
        modal: {
            showProfileModal: true
        }
    };

    const myUser = initialState.login.myUser;

    act(() => {
        render(<div><ProfileModal myUser={myUser}/></div>, { initialState });
    });

    const endPointUrl = `${process.env.REACT_APP_API_URL}/accounts/${myUser.id}`
    mock.onPut(endPointUrl)
        .reply(400, "Failed");

    const inputs = screen.getAllByRole('textbox');
    const submit = screen.getByRole('button');

    const nameInput = inputs[0];

    fireEvent.change(nameInput, { target: { value: 'updatedName' } });
    fireEvent.click(submit);

    await waitFor(() => {
        expect(screen.queryByText('Failed')).not.toBeInTheDocument();
    });

});


