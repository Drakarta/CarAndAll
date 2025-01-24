import "../styles/footer.css";

export default function Footer() {
  return (
    <div className="footer-container">
      <div className="footer">
        <div className="footer-top">
          <h1>CarAndAll</h1>
          <p>Car and All is a trusted rental service specializing in cars, caravans, and campers. Whether you're planning a road trip, family vacation, or an outdoor adventure, Car and All offers a wide range of vehicles to suit your needs. From compact cars for city travel to spacious campers for extended journeys, the service ensures reliability, comfort, and convenience. With flexible rental terms and a commitment to customer satisfaction, Car and All makes it easy to hit the road and explore with confidence.</p>
        </div>
        <div className="footer-bottom">
          <div>
            <h2>Company</h2>
            <ul>
              <li>About</li>
              <li>Careers</li>
              <li>Blog</li>
            </ul>
          </div>
          <div>
            <h2>Contact</h2>
            <ul>
              <li>Phone: 123-456-7890</li>
              <li>Email:</li>
            </ul>
          </div>
          <div>
            <h2>Policies</h2>
            <ul>
              <li>Privacy</li>
            </ul>
          </div>
        </div>
      </div>
    </div>
  )
}
