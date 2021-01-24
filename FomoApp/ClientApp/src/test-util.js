// test-utils.js

import { Provider } from 'react-redux'
import React from 'react'
import { configureStore } from '@reduxjs/toolkit';
import { reducers } from './app/Store';
import { render as rtlRender } from '@testing-library/react'

function render(
  ui,
  {
    initialState,
    store = configureStore({ ...reducers, preloadedState: initialState}),
    ...renderOptions
  } = {}
) {
  function Wrapper({ children }) {
    return <Provider store={store}>{children}</Provider>
  }
  return rtlRender(ui, { wrapper: Wrapper, ...renderOptions })
}

// re-export everything
export * from '@testing-library/react'

// override render method
export { render }
