import { Route, Routes } from 'react-router';
import AppRoutes from './AppRoutes';
import { ThemeProvider } from './components/ThemeProvider';

function App() {
    return (
        <>
            <ThemeProvider>
                <Routes>
                    {AppRoutes.map((route, index) => {
                        const { element, path, index: isIndex, ...rest } = route;
                        return <Route key={index} path={path} index={isIndex} element={element} {...rest} />;
                    })}
                </Routes>
            </ThemeProvider>
        </>
    );
}

export default App;
