import rootReducer  from './rootReducer';

test("Should combine the correct reducers", () => {
    expect(rootReducer).not.toBeNull(rootReducer.portfolioReducer);
    expect(rootReducer).not.toBeNull(rootReducer.symbolReducer);
    expect(rootReducer).not.toBeNull(rootReducer.loginReducer);
});