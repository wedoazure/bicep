// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import React from 'react';
import ReactDOM from 'react-dom';
import { Container, Row, Spinner } from 'react-bootstrap';
import 'bootstrap/dist/css/bootstrap.min.css';

import './index.css';
import { initializeInterop } from './helpers/lspInterop';
import { Playground } from './components/Playground';
import { createLanguageClient } from './helpers/client';

ReactDOM.render(
  <div className="app-container">
    <Playground />
  </div>,
  document.getElementById('root')
);