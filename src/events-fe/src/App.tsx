import './App.css'
import { AnalyzePage } from './views/analyzePage'
import { BrowserRouter, Route, Routes } from 'react-router-dom'
import { NavBar } from './components/navbar'
import { RecordingPage } from './views/recordingPage'

function App() {

  return (
    <BrowserRouter>
      <NavBar />
      
      <Routes>
        <Route path="/" element={<RecordingPage />} />
        
        <Route path="/analyze" element={<AnalyzePage />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App
