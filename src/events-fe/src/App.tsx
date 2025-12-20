import './App.css'
import { AnalyzePage } from './views/analyzePage'
import { BrowserRouter, Route, Routes } from 'react-router-dom'
import { RecordingPage } from './views/recordingPage'
import Navbar from './components/Navbar/navbar';

function App() {

  return (
    <BrowserRouter>
      <Navbar />
      
      <Routes>
        <Route path="/" element={<RecordingPage />} />
        
        <Route path="/analyze" element={<AnalyzePage />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App
