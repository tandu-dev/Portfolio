import { render, screen } from '@testing-library/react';
import TopBar from './topbar';

test('renders menu', () => {
    render(<TopBar />);
    const linkElement = screen.getByRole("menu");
    expect(linkElement).toBeInTheDocument();
});