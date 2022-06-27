import { render, screen } from '@testing-library/react';
import App from './App';


test('renders topbar', () => {
    render(<App />);
    screen.debug();
    const linkElement = screen.getByRole("menu");
    expect(linkElement).toBeInTheDocument();
});
