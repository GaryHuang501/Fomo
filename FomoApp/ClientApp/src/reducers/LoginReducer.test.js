import { loginReducer } from './LoginReducer';
import { unauthorizedResponse } from '../actions/LoginActions';

test("Should return isUnauthorized state as false by default", () => {
    var state = loginReducer(undefined, undefined);
    expect(state.isUnauthorized).not.toBeTruthy();
});

test("Should return isUnauthorized state as true by when unauthorizedResponse", () => {
    var state = loginReducer({ isUnauthorized: false }, unauthorizedResponse());
    expect(state.isUnauthorized).toBeTruthy();
});