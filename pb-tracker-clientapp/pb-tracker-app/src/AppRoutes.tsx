import LoginPage from './app/login/page';
import RegisterPage from './app/register/page';
import { RegisterForm } from './components/forms/register-form';

interface RouteConfiguration {
    path?: string;
    index?: boolean;
    element: React.ReactElement;
}

const AppRoutes: RouteConfiguration[] = [
    {
        path: '/login',
        element: <LoginPage />,
    },
    {
        path: '/register',
        element: <RegisterPage />,
    },
];

export default AppRoutes;
